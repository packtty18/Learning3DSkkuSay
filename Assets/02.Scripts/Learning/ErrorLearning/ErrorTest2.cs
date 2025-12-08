using UnityEngine;

public class ErrorTest2 : MonoBehaviour
{
    private void Start()
    {
        //missingReferenceException : 삭제된 게임 오브젝트에 접근할경우
        Destroy(gameObject);
        Debug.Log(gameObject.name);

        //IndexOutOfRangeException : 배열이나 리스트에서 유효하지 않은 인덱스 접근시
        int[] nums = new int[10];
        Debug.Log(nums[11]);

        //DevideByZeroException : 0으로 나눠질경우 에러
        int num = 10;
        int zero = 0;
        Debug.Log(num/zero);
    }
}
