__kernel void brahmaKernel (__global int * buf)
{int i = 1 ;
 for (int i0 = 0 ; (i0 <= (i + 1)) ; i0 ++)
 {int i1 = (i0 + 2) ;
  buf [i1] = 2 ;} ;}