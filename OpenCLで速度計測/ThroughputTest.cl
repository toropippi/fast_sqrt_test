//sqrt or fast_sqrt_double or fast_sqrt_float
#define Xsqrt sqrt
//float or double
#define floatT float


double fast_resqrt_double(const double x)
{
    union {
        long long i;
        double d;
    } converter;
	converter.d=x;
	
    double xhalf = 0.5 * x;//1cycles
	converter.i = 0x5FE6EB50C7B537AAl - (converter.i >> 1);//2cycles
    double ux = converter.d;
    ux = ux * (1.5 - xhalf * ux * ux);//3cycles
    ux = ux * (1.5 - xhalf * ux * ux); //3cycles
    return ux;
}

double fast_sqrt_double(const double x)
{
    return x * fast_resqrt_double(x);//1cycles
}



float fast_resqrt_float(const float x)
{
    union {
        int i;
        float f;
    } converter;
	converter.f=x;
	
    float xhalf = 0.5f * x;//1cycles
	converter.i = 0x5F3759DF - (converter.i >> 1);//2cycles
    float ux = converter.f;
    ux = ux * (1.5f - xhalf * ux * ux);//3cycles
    ux = ux * (1.5f - xhalf * ux * ux); //3cycles
    return ux;
}

float fast_sqrt_float(const float x)
{
    return x * fast_resqrt_float(x);//1cycles
}



__kernel void mainkernel(__global float* dA,__global float* dB,__global float* dC,int loopn)
{
    uint id = get_global_id(0);
    floatT aa = dA[id] + (floatT)id;
    floatT bb = dB[id] + (floatT)id;
    
    for (uint i = 0; i < loopn; i++)
    {
        bb = Xsqrt(bb) + aa;
        bb = Xsqrt(bb) + aa;
        bb = Xsqrt(bb) + aa;
        bb = Xsqrt(bb) + aa;
    }
    dC[id] = (float)bb;
}


