using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FastSqrtTest : MonoBehaviour
{
    public ComputeShader shader;

    float[] host_A;
    float[] host_B;
    float[] host_C;

    ComputeBuffer A;
    ComputeBuffer B;
    ComputeBuffer C;

    int k;
    int cnt;
    int n;
    void Start()
    {
        n = 1024 * 1024 * 256;//1GB
        host_A = new float[n];
        host_B = new float[n];
        host_C = new float[n];

        A = new ComputeBuffer(host_A.Length, sizeof(float));
        B = new ComputeBuffer(host_B.Length, sizeof(float));
        C = new ComputeBuffer(host_C.Length, sizeof(float));

        k = shader.FindKernel("CSMain");

        // host to device
        A.SetData(host_A);
        B.SetData(host_B);

        //�������Z�b�g
        shader.SetBuffer(k, "dA", A);
        shader.SetBuffer(k, "dB", B);
        shader.SetBuffer(k, "dC", C);
        shader.SetInt("loopnum", 32);

        // GPU�Ōv�Z�A����J�[�l���N���Ŕ��������̃I�[�o�[�w�b�h�������ŏ�������
        shader.Dispatch(k, n / 256 / 1024, 1024, 1);
        // device to host
        C.GetData(host_C, 0, 0, 4);

        
        Debug.Log("GPU��Ōv�Z��������");
        for (int i = 0; i < 4; i++)
        {
            Debug.Log(host_A[i] + ", " + host_B[i] + ", " + host_C[i]);
        }
        cnt = 0;
    }



    void Update()
    {
        if (cnt == 140)
        {
            //���Ԍv���J�n
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            // GPU�Ōv�Z
            shader.Dispatch(k, n / 256 / 1024, 1024, 1);
            C.GetData(host_C, 0, 0, 4);

            //���Ԍv���I��
            sw.Stop();
            //sw����debug�\��//https://qiita.com/Kosen-amai/items/81efaf815b48ab9ffbb6
            TimeSpan ts = sw.Elapsed;
            Debug.Log($"�@{ts}");
            Debug.Log($"�@{sw.ElapsedMilliseconds}�~���b");
        }

        cnt++;
    }


    void OnDestroy()
    {
        A.Release();
        B.Release();
        C.Release();
    }

}