/*
 * queue.h
 *
 *  Created on: Oct. 31, 2021
 *      Author: Ryan
 */

#ifndef QUEUE_H_
#define QUEUE_H_


struct circularBuffer
{
    unsigned int size;
    unsigned char buffer[50];
    unsigned int start;
    unsigned int stop;
    unsigned int numItems;
};

typedef volatile struct circularBuffer queue;

int isFull(queue* q)
{
    return (q->numItems >= q->size);
}
int initializeBuffer(queue* q)
{
    q->size = 50;
    q->start = 0;
    q->stop = 0;
    q->numItems = 0;
    return 0;
}

int enqueue(queue* q, unsigned char val)
{
    if (isFull(q))
    {
        return -1;
    }
    q->buffer[q->stop] = val;
    q->stop = (q->stop + 1) % q->size;
    q->numItems ++;
    return 0;
}

int dequeue(queue* q, unsigned char* val)
{
    if (q->numItems <= 0)
    {
        return -1;
    }
    *val = q->buffer[q->start];
    q->start = (q->start + 1) % q->size;
    q->numItems --;
    return 0;
}
unsigned char peek(queue* q)
{
    return q->buffer[q->start];
}



#endif /* QUEUE_H_ */
