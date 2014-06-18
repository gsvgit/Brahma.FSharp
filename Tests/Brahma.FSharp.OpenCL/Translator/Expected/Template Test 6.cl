int f (private int x, private int y)
{int x0 = x ;
 return (x0 + y) ;}
__kernel void brahmaKernel (__global int * buf)
{buf [0] = f (7, 8) ;}