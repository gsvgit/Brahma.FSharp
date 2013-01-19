__kernel void brahmaKernel (__global int * buf)
{while ((((buf [0] < 5) & (((buf [1] < 6) & 1) | (buf [2] > 2))) | 0))
 {buf [0] = 1 ;} ;}