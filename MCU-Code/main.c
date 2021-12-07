#include <msp430.h> 
#include "queue.h"
#include "stepper.h"

queue buf;
volatile int packet_flag = 0;
volatile int timerUART_flag = 0;
volatile unsigned int dataword = 0;
volatile unsigned char cmd, db1, db2, esc;

volatile unsigned char stepper_state = 0;
volatile unsigned int stepper_dir = 0;
volatile unsigned int stepper_timestep = 0x8000;

volatile unsigned int commanded_stepper_position = 0; //in half-steps, 400 half-steps per rev
volatile unsigned int actual_stepper_position = 0;

const double CONVERT_DEG_TO_HALFSTEP = 400.0/360.0;

/**
 * main.c
 */
int main(void)
{
    WDTCTL = WDTPW | WDTHOLD;   // stop watchdog timer

    // Configure clocks
    CSCTL0 = 0xA500;                        // Write password to modify CS registers
    CSCTL1 = DCOFSEL0 + DCOFSEL1;           // DCO = 8 MHz
    CSCTL2 = SELM0 + SELM1 + SELA0 + SELA1 + SELS0 + SELS1; // MCLK = DCO, ACLK = DCO, SMCLK = DCO
    CSCTL3 = 0;                     //reset the dividers

    ////////
    //UART//
    ////////

    // Configure ports for UCA1
    P2SEL0 &= ~(BIT5 + BIT6);
    P2SEL1 |= BIT5 + BIT6;

    // Configure UCA1
    UCA1CTLW0 = UCSSEL0;
    UCA1BRW = 52;
    UCA1MCTLW = 0x4900 + UCOS16 + UCBRF0;
    UCA1IE |= UCRXIE;

    //Set up timer to 40ms: 8MHz / 8 * 40ms = 40000
    TA0CCTL0 = CCIE;                          // TACCR0 interrupt enabled
    TA0CCR0 = 40000;
    TA0CTL = TASSEL_2 + MC_2 + ID__8;                 // SMCLK, continuous mode, div 8

    ////////////
    //DC Motor//
    ////////////

    //Set up timer B
    TB2CTL |= TBSSEL__SMCLK;    //SMCLK source
    TB2CTL |= CNTL__16;         //16 bit clock - 122.07 Hz
    TB2CTL |= MC__UP;           //up mode
    TB2CTL |= TBCLR;            //reset clock
    TB2CCR0 = 0xFFFF;           //TB2CL0

    //Set up timer B2.1 with an initial duty cycle
    TB2CCR1 = 0x1FFF;       //TB2CL1
    TB2CCTL1 |= OUTMOD_3;   // PWM set/reset

    //P2.1 - PWMA
    P2DIR |= BIT1;      //P2.1 output
    P2SEL0 |= BIT1;     //P2.1 options select - output TB2.1
    P2SEL1 &= ~BIT1;

    //P3.6 - AIN2
    //P3.7 - AIN1
    P3DIR |= BIT6 + BIT7;
    P3SEL0 &= ~(BIT6 + BIT7);
    P3SEL1 &= ~(BIT6 + BIT7);

    // Set P3.6 to high and P3.7 to low - clockwise
    P3OUT |= BIT6;
    P3OUT &= ~BIT7;

    /////////////////
    //Stepper Motor//
    /////////////////

    //Set up timer A1 to run at 8MHz, with it causing an interrupt according to stepper_timestep
    TA1CCTL0 = CCIE;                          // TACCR0 interrupt enabled
    TA1CCR0 = stepper_timestep;
    TA1CTL = TBSSEL__SMCLK + MC__CONTINUOUS + ID__1;         // SMCLK, continuous mode, div 1

    //P1.5 - AIN1 - TB0.2
    //P1.4 - AIN2 - TB0.1
    //P3.5 - BIN1 - TB1.2
    //P3.4 - BIN2 - TB1.1

    const unsigned int stepperDutyCycle = 25;

    //Set up timer B0 for AIN
    TB0CTL |= TBSSEL__SMCLK + CNTL__16 + MC__UP + ID__8;    //SMCLK source, 16 bit clock - 122.07 Hz, up mode, div8
    //for 10kHz, 20% duty cycle
    TB0CCR0 = 100;
    TB0CCR1 = stepperDutyCycle;               //AIN1
    TB0CCR2 = stepperDutyCycle;               //AIN2
    TB0CCTL1 |= OUTMOD_7;       // PWM reset/set
    TB0CCTL2 |= OUTMOD_7;       // PWM reset/set
    P1DIR |= BIT4 + BIT5;
    P1SEL0 |= BIT4 + BIT5;
    P1SEL1 &= ~(BIT4 + BIT5);

    //Set up timer B1 for BIN
    TB1CTL |= TBSSEL__SMCLK + CNTL__16 + MC__UP + ID__8;    //SMCLK source, 16 bit clock - 122.07 Hz, up mode, div8
    //for 10kHz, 20% duty cycle
    TB1CCR0 = 100;
    TB1CCR1 = stepperDutyCycle;               //BIN1
    TB1CCR2 = stepperDutyCycle;               //BIN2
    TB1CCTL1 |= OUTMOD_7;       // PWM reset/set
    TB1CCTL2 |= OUTMOD_7;       // PWM reset/set
    P3DIR |= BIT4 + BIT5;
    P3SEL0 |= BIT4 + BIT5;
    P3SEL1 &= ~(BIT4 + BIT5);


    /////////////////
    //Switch inputs//
    /////////////////

    P3DIR &= ~(BIT0 + BIT1 + BIT2 + BIT3);
    //inputs hands_p1, hands_p2, LS_p1, LS_p2


    //Enable pulldown resistor - switches to be placed btwn VCC & input pin
    P3REN |= (BIT0 + BIT1 + BIT2 + BIT3);
    P3OUT &= ~(BIT0 + BIT1 + BIT2 + BIT3);


    /*
    //Set P3.0 to get interrupted from a rising edge (i.e. an interrupt occurs when the user lets go of the button).
    P3IES &= ~BIT0;
    // Enable local and global interrupts.
    P3IE |= BIT0;
    */
    //Configure PJ.2 as an output (this is connected via wire to LED4)
    PJDIR |= BIT2;
    PJSEL0 &= ~( BIT2);
    PJSEL1 &= ~( BIT2);

    initializeBuffer(&buf);
    // global interrupt enable
    _EINT();

    while(1)
    {
        while(!packet_flag && !timerUART_flag){}
        //packet flag is now set - we have an item in the queue


        if (timerUART_flag)
        {
            unsigned char data[5];

            data[0] = 255;
            data[1] = BIT5; //sensor data bit
            data[2] = P3IN; //hands_p1, hands_p2, limswitch_1, limswitch_2
            //hands are a bit glitchy through the AND
            data[3] = 0;
            data[4] = 0;
            sendArrToUART(data,5);

            timerUART_flag = 0;
            continue;
        }
        else if (packet_flag)
        {
            unsigned char val = 0;
            while (val != 255 && !dequeue(&buf,&val)); //dequeue until a start bit, or until queue is empty

            if (buf.numItems == 0 && val != 255) //queue is empty and no start bit found, return to searching the queue
            {
                packet_flag = 0;
                continue;
            }
            else if (val == 255) //start bit found, we need 4 more bits from the queue
            {
                int i;
                unsigned char data[4];
                for (i = 0; i < 4; i ++)
                {
                    //will get stuck until data - should check num items in queue
                    while(dequeue(&buf, data + i )); //try dequeuing until we get data
                }
                if (i < 4) //not enough data in queue
                {
                    packet_flag = 0;
                    continue;
                }
                cmd = data[0];
                db1 = data[1];
                db2 = data[2];
                esc = data[3];

                sendToUART(255);
                sendToUART(cmd & BIT7);
                sendToUART(db1);
                sendToUART(db2);
                sendToUART(esc);

                //if escape = 1, db2 = 255
                //if escape = 2, db1 = 255
                //if escape = 4, cmd = 255
                cmd |= 255 * ( (esc & BIT2) >> 2);
                db1 |= 255 * ( (esc & BIT1) >> 1);
                db2 |= 255 * (esc & BIT0);

                dataword = (db1 << 8) + db2;

                if (cmd & BIT0)     // DC motor CCW
                {
                    TB2CCR1 = 0xFFFF - dataword; //higher value -> faster
                    P3OUT |= BIT6;
                    P3OUT &= ~BIT7;
                }
                else                //DC motor CW
                {
                    TB2CCR1 = 0xFFFF - dataword; //higher value -> faster
                    P3OUT &= ~BIT6;
                    P3OUT |= BIT7;
                }
                if (cmd & BIT1)     //stepper CCW
                {
                    stepper_timestep = 0xFFFF - (dataword & 0xEFFF); //higher value -> faster
                    stepper_dir = 0;
                }
                else                //stepper CW
                {
                    stepper_timestep = 0xFFFF - (dataword & 0xEFFF); //higher value -> faster
                    stepper_dir = 1;
                }
                if (cmd & BIT2)     //move to given position
                {
                    commanded_stepper_position = dataword * CONVERT_DEG_TO_HALFSTEP;
                    //0 to 359 degrees in standard position
                    if ((actual_stepper_position - commanded_stepper_position + 400) % 400 < 200)
                    {
                        stepper_dir = 1; //CW
                    }
                    else
                    {
                        stepper_dir = 0; //CCW
                    }
                }
                /*
                if (cmd & BIT2)     //single step, stops timer
                {
                    TA1CTL &= ~MC_2;
                    rotate_stepper(stepper_dir);
                }
                if (cmd & BIT3)     //resume continuous
                {
                    TA1CTL |= MC_2;
                }*/
                packet_flag = 0;
            }
            continue;
        }
    }
}

/*
//interrupt service routine to toggle LED8 when P3.0 provides a rising edge.
#pragma vector = PORT3_VECTOR
__interrupt void S1_interrupt (void)
{
    //check which ifg was raised P3IV
    PJOUT ^= BIT2;
    P3IFG &= ~BIT0;
}*/



void sendToUART(unsigned char byte)
{
    while ((UCA1IFG & UCTXIFG)==0);
    UCA1TXBUF = byte;
}

void sendArrToUART(unsigned char data[], unsigned int size)
{
    int i;
    for (i=0; i<size; i++)
    {
        sendToUART(data[i]);
    }
}


void sendUARTerror()
{
    static const unsigned char errmsg[6] = "ERR\r\n";
    unsigned int i;
    for (i = 0; i < 4; i++)
    {
        sendToUART(errmsg[i]);
    }
}

#pragma vector = USCI_A1_VECTOR
__interrupt void USCI_A1_ISR(void)
{
    unsigned char RxByte;
    RxByte = UCA1RXBUF;

    if(enqueue(&buf, RxByte))
    {
        sendUARTerror();
    }
}

//for stepping stepper
#pragma vector = TIMER1_A0_VECTOR
__interrupt void Timer_1A0 (void)
{
    timerUART_flag = 1;
    if (commanded_stepper_position == actual_stepper_position)
    {
        return;
    }
    if (stepper_dir == 1)//CW
    {
        if (actual_stepper_position == 0)
        {
            actual_stepper_position = 399;
        }
        else
        {
            actual_stepper_position --;
        }
    }
    else //stepper_dir = 0 -> CCW
    {
        if (actual_stepper_position == 399)
        {
            actual_stepper_position = 0;
        }
        else
        {
            actual_stepper_position++;
        }
    }
    TA1CCR0 += stepper_timestep;
    rotate_stepper(stepper_dir);
}

//for UART input to buffer
#pragma vector = TIMER0_A0_VECTOR
__interrupt void Timer_0A0 (void)
{
    TA0CCR0 += 40000;                         // Add Offset to TACCR0
    if (buf.numItems > 0)
    {
        packet_flag = 1;
    }
}
