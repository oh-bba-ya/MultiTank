using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawningCoin : Coin
{
    public event Action<RespawningCoin> OnCollected;

    private Vector3 _previousPosition;

    private void Update()
    {
        if (_previousPosition != transform.position)
        {
            Show(true);
        }

        _previousPosition = transform.position;
    }

    public override int Collect()
    {
        // ������ �ƴ϶��..
        if(!IsServer)
        {
            // ������ ��Ȱ��ȭ..
            Show(false); 
            return 0;
        }


        // �̹� ������ �����̶��..
        if(alreadyCollected) { return 0; }

        // ���� �Ϸ�
        alreadyCollected = true;

        OnCollected?.Invoke(this);

        // ����Value ��ȯ
        return coinValue;


    }

    public void Reset()
    {
        alreadyCollected = false;
    }


}
