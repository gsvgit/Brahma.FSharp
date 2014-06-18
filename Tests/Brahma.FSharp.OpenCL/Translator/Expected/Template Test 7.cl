int x (private int y0)
{return (6 - y0) ;}
int f (private int y)
{return x (y) ;}
__kernel void brahmaKernel (__global int * buf)
{buf [0] = f (7) ;}