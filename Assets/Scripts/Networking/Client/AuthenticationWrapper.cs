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

        // 시도횟수.. 만일 시도횟수를 제한하지 않는다면 인증될 때까지 반복된다.
        // 시도횟수를 설정함으로서 시도횟수를 초과해도 인증 되지 않는다면
        // 에러로 취급한다.
        int reTries = 0;
        while (AuthState == AuthState.Authenticating && reTries < maxRetries)
        {
            // try-catch를 사용함으로서 예외상황에서도 프로그램이 계속 수행될 수 있도록 한다.
            try
            {
                // UGS의 인증 시도..
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

                if (AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized)
                {
                    AuthState = AuthState.Authenticated;
                    break;
                }

            }
            catch(AuthenticationException ex)   // 인증실패..
            {
                Debug.LogError(ex);
                AuthState = AuthState.Error;
            }
            catch(RequestFailedException exeption)   // 요청 실패..
            {
                Debug.LogError(exeption);
                AuthState = AuthState.Error;
            }


            reTries++;
            // 실패하면 1초 동안 기다림
            await Task.Delay(1000);
        }

        // maxRetries * 1초 만큼의 시간을 거쳐 인증에 실패하면..
        if(AuthState != AuthState.Authenticated)
        {
            Debug.LogWarning($"플레이어 로그인 실패 {reTries}번 시도..");
            // 시간초과
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
