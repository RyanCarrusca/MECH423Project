/*
 * stepper.h
 *
 *  Created on: Nov. 4, 2021
 *      Author: Ryan
 */

#ifndef STEPPER_H_
#define STEPPER_H_

extern volatile unsigned char stepper_state;

static const unsigned char stepper_phases_p15[8] = {OUTMOD_7, OUTMOD_7, OUTMOD_5, OUTMOD_5, OUTMOD_5, OUTMOD_5, OUTMOD_5, OUTMOD_7};
static const unsigned char stepper_phases_p14[8] = {OUTMOD_5, OUTMOD_5, OUTMOD_5, OUTMOD_7, OUTMOD_7, OUTMOD_7, OUTMOD_5, OUTMOD_5};
static const unsigned char stepper_phases_p35[8] = {OUTMOD_5, OUTMOD_7, OUTMOD_7, OUTMOD_7, OUTMOD_5, OUTMOD_5, OUTMOD_5, OUTMOD_5};
static const unsigned char stepper_phases_p34[8] = {OUTMOD_5, OUTMOD_5, OUTMOD_5, OUTMOD_5, OUTMOD_5, OUTMOD_7, OUTMOD_7, OUTMOD_7};


/*
 * Rotates the stepper 1 step
 * @param dir: 0 is CCW (backward), 1 is CW (forward)
 *
 * outputs to analog input pins of the motor driver, either PWM signal for on, or low signal for off
 * AIN1 - P1.5 - TB0.2
 * AIN2 - P1.4 - TB0.1
 * BIN1 - P3.5 - TB1.2
 * BIN2 - P3.4 - TB1.1
 */
void inline rotate_stepper(int dir)
{
    stepper_state = (stepper_state + (dir ? 1 : 7)) % 8;

    //Stop the clocks
    //TB0CTL &= ~MC__UP;
    //TB1CTL &= ~MC__UP;

    //TB0CCTL1 &= ~OUTMOD_7;              // clear outmode
    //TB0CCTL2 &= ~OUTMOD_7;              // clear outmode
    TB0CCTL1 = stepper_phases_p14[stepper_state];     // PWM reset/set AIN1 - P1.5
    TB0CCTL2 = stepper_phases_p15[stepper_state];     // PWM reset/set AIN2 - P1.4

    //TB1CCTL1 &= ~OUTMOD_7;              // clear outmode
    //TB1CCTL2 &= ~OUTMOD_7;              // clear outmode
    TB1CCTL1 = stepper_phases_p34[stepper_state];     // PWM reset/set BIN1 - P3.5
    TB1CCTL2 = stepper_phases_p35[stepper_state];     // PWM reset/set BIN2 - P3.4

    //Start the clocks
    //TB0CTL |= TBCLR + MC__UP;
    //TB1CTL |= TBCLR + MC__UP;
}



#endif /* STEPPER_H_ */
