using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DealDamageOnContact : MonoBehaviour
{
    [SerializeField] private int damage = 5;

    private ulong _ownerClientId;
    public void SetOwner(ulong ownerClientId)
    {
        _ownerClientId = ownerClientId;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 콜라이더가 연결되어 있는 리지드바디가 null이면.. 리턴
        if (other.attachedRigidbody == null)
        {
            Debug.Log("NUll");
            return;
        }

        if (other.attachedRigidbody.TryGetComponent<NetworkObject>(out NetworkObject netObj))
        {
            // 내가 쏜 총알에 내가 충돌하면..
            if(_ownerClientId == netObj.OwnerClientId)
            {
                Debug.Log("나와 충돌");
                return;
            }
        }



        if (other.attachedRigidbody.TryGetComponent<Health>(out Health healthComp))
        {
            Debug.Log("데미지");
            healthComp.TakeDamage(damage);
        }
        else
        {
            Debug.Log("Health 콤프가 없는건가?");
        }
    }
}
