using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public static class AuthenticationWrapper
{
    public static AuthState AuthState { get; private set; } = AuthState.NotAuthenticated;

    public static async Task<AuthState> DoAuth(int maxRetries = 5)
    {
        if(AuthState == AuthState.Authenticated)
        {
            return AuthState;
        }

        if(AuthState == AuthState.Authenticating)
        {
            Debug.LogWarning("Already authenticating");
            await Authenticating();
            return AuthState;
        }

        await SignInAnonymouslyAsync(maxRetries);


        return AuthState;
    }

    private static async Task<AuthState> Authenticating()
    {
        while (AuthState == AuthState.Authenticated || AuthState == AuthState.NotAuthenticated)
        {
            await Task.Delay(200);
        }

        return AuthState;
    }

    private static async Task SignInAnonymouslyAsync(int maxRetries)
    {
        AuthState = AuthState.Authenticating;

        // �õ�Ƚ��.. ���� �õ�Ƚ���� �������� �ʴ´ٸ� ������ ������ �ݺ��ȴ�.
        // �õ�Ƚ���� ���������μ� �õ�Ƚ���� �ʰ��ص� ���� ���� �ʴ´ٸ�
        // ������ ����Ѵ�.
        int reTries = 0;
        while (AuthState == AuthState.Authenticating && reTries < maxRetries)
        {
            // try-catch�� ��������μ� ���ܻ�Ȳ������ ���α׷��� ��� ����� �� �ֵ��� �Ѵ�.
            try
            {
                // UGS�� ���� �õ�..
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

                if (AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized)
                {
                    AuthState = AuthState.Authenticated;
                    break;
                }

            }
            catch(AuthenticationException ex)   // ��������..
            {
                Debug.LogError(ex);
                AuthState = AuthState.Error;
            }
            catch(RequestFailedException exeption)   // ��û ����..
            {
                Debug.LogError(exeption);
                AuthState = AuthState.Error;
            }


            reTries++;
            // �����ϸ� 1�� ���� ��ٸ�
            await Task.Delay(1000);
        }

        // maxRetries * 1�� ��ŭ�� �ð��� ���� ������ �����ϸ�..
        if(AuthState != AuthState.Authenticated)
        {
            Debug.LogWarning($"�÷��̾� �α��� ���� {reTries}�� �õ�..");
            // �ð��ʰ�
            AuthState = AuthState.TimeOut;
        }

    }

}

public enum AuthState
{
    NotAuthenticated,
    Authenticating,
    Authenticated,
    Error,
    TimeOut
}
