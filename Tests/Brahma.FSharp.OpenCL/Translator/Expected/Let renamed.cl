int f (private int x)
{int g = (1 + x) ;
 return g ;}
__kernel void brahmaKernel (__global int * buf)
{buf [0] = f (1) ;}