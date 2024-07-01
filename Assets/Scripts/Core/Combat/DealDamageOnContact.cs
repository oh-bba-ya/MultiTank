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
        // �ݶ��̴��� ����Ǿ� �ִ� ������ٵ� null�̸�.. ����
        if (other.attachedRigidbody == null)
        {
            return;
        }

        if (other.attachedRigidbody.TryGetComponent<NetworkObject>(out NetworkObject netObj))
        {
            // ���� �� �Ѿ˿� ���� �浹�ϸ�..
            if(_ownerClientId == netObj.OwnerClientId)
            {
                return;
            }
        }



        if (other.attachedRigidbody.TryGetComponent<Health>(out Health healthComp))
        {
            healthComp.TakeDamage(damage);
        }
    }
}
