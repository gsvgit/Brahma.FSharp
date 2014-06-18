int g (private int x2, private int m)
{return (m + x2) ;}
int f (private int x, private int y)
{int y0 = y ;
 int y1 = y0 ;
 return g (x, y1) ;}
__kernel void brahmaKernel (__global int * buf)
{buf [0] = f (1, 7) ;}