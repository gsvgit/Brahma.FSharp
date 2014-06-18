int g (__global int * m, private int k)
{return ((k + m [0]) + m [1]) ;}
int z (private int a, __global int * m, private int t)
{return ((m [2] + a) - t) ;}
int y (__global int * m, private int l, private int n, private int a)
{int x0 = ((5 - n) + g (m, 4)) ;
 return z (a, m, ((a + x0) + l)) ;}
int x (__global int * m, private int n)
{int l = m [9] ;
 int r = y (m, l, n, 6) ;
 return (r + m [3]) ;}
__kernel void brahmaKernel (__global int * m)
{int p = m [0] ;
 m [0] = x (m, 7) ;}