int x (private int y, private int z)
{return (y + z) ;}
int f (private int a, private int b)
{return x (a, b) ;}
__kernel void brahmaKernel (__global int * buf)
{buf [0] = f (1, 7) ;}