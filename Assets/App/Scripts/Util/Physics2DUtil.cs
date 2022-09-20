using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Physics2DUtil
{
    /// <summary>
    /// �w�肵���b����̎��R�����������W���v�Z����
    /// </summary>
    /// <param name="vector"></param>
    /// <param name="seconds"></param>
    /// <returns></returns>
    public static Vector2 CalcFreeFallPosFewSeconds(Vector2 vector, float seconds)
    {
        float halfGravity = -Physics2D.gravity.y * 0.5f;
        float gravAcceleration = halfGravity * Mathf.Pow(seconds, 2.0f);
        float simulatedVectorY = vector.y - gravAcceleration;

        vector.x = seconds * vector.x;
        vector.y = seconds * simulatedVectorY;

        return vector;
    }

    /// <summary>
    /// ����̍����ɓ���̑��x�œ��B���邽�߂̏����x���v�Z���� (�d�͂���A��C��R�Ȃ�)
    /// </summary>
    /// <param name="height"></param>
    /// <returns></returns>
    public static float CalcInitialVelocityToReachHeight(float height, float speed, float gravityScale)
    {
        return Mathf.Sqrt(Mathf.Pow(speed, 2.0f) - (2 * Physics2D.gravity.y * gravityScale * height));
    }
}
