using System;
using UnityEngine;

public class TryCatchFinallyTest : MonoBehaviour
{
    /*
     * 예외 : 런타임에서 발생하는 오류
     * 
     * try - catch : 예외를 처리하는 기본 문법
     */

    public int Age;

    private void Start()
    {
        //아래 문법은 인덱스의 범위를 벗어나 예외 발생
        //다른 요소에도 영향을 주어 프로그램의 동작에 영향을 준다.


        //베스트는 알고리즘을 잘 처리하는 것이지만 현실적으로 완벽한 코드는 불가능
        int[] numbers = new int[32];

        try
        {
            //예외가 발생할만한 코드 작성
            DebugManager.Instance.Log("Try");
            int index = 75;
            numbers[index] = 1;
        }
        catch (Exception e)
        {
            //예외가 일어날경우 코드 작성
            DebugManager.Instance.Log("Catch");
            DebugManager.Instance.LogWarning($"{e} 예외 발생");

            int index = numbers.Length -1;
            numbers[index] = 1;
        }
        finally
        {
            //옵션
            //정상이거나 예외 처리 후 실행할 코드
            DebugManager.Instance.Log("Finally");
        }

        //하지만 try-catch는 안쓰는게 좋음 
        // - 성능 저하 (주된 이유)
        // - 잘못된 알고리즘 발생 가능성 증가

        // - 써야하는 경우 = 내가 제어할수 없을 경우
        //  -> 네트워크 접근(로그인/로그아웃, 서버)
        //  -> 파일 접근 (파일 용량, 파일명, 권한)
        //  -> DB 접근


        //Log와 Exception
        if (Age <= 0)
        {
            Debug.Log("에러발생"); //로그만 남기고 다음으로 넘어감
            throw new Exception("에러발생"); //에러로그를 남기면서 다음으로 넘어가지 않음
        }
    }

}
