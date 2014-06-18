int x (private int n, private int b)
{int t = 0 ;
 return ((n + b) + t) ;}
__kernel void brahmaKernel (__global int * buf)
{int p = 9 ;
 buf [0] = x (7, 9) ;}