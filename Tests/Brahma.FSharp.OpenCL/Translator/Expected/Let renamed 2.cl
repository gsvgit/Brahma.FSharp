int g (private int q, private int w)
{return ((1 + q) + w) ;}
int t (private int p)
{return (7 - p) ;}
int f (private int m, private int k)
{return (g (1, 2) - ((m * k) / t (53))) ;}
__kernel void brahmaKernel (__global int * buf)
{buf [0] = f (1, 4) ;}