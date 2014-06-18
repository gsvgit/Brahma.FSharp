int g (private int m)
{return (m + 1) ;}
int f (private int y)
{int y0 = y ;
 int y1 = y0 ;
 return g (y1) ;}
__kernel void brahmaKernel (__global int * buf)
{buf [0] = f (7) ;}