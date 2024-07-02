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
        // 서버가 아니라면..
        if(!IsServer)
        {
            // 렌더러 비활성화..
            Show(false); 
            return 0;
        }


        // 이미 수집된 코인이라면..
        if(alreadyCollected) { return 0; }

        // 수집 완료
        alreadyCollected = true;

        OnCollected?.Invoke(this);

        // 코인Value 반환
        return coinValue;


    }

    public void Reset()
    {
        alreadyCollected = false;
    }


}
