using UnityEngine;
using System.Collections;

public class InterpolatedTransform : MonoBehaviour
{
    private Vector3[] m_lastPositions; 
    private int m_newTransformIndex; 

    void OnEnable()
    {
        ForgetPreviousTransforms();
    }

    public void ForgetPreviousTransforms()
    {
        m_lastPositions = new Vector3[2];
        m_newTransformIndex = 0;
    }

    void FixedUpdate()
    {
        Vector3 mostRecentTransform = m_lastPositions[m_newTransformIndex];
        transform.position = mostRecentTransform;
    }

    public void LateFixedUpdate()
    {
        m_newTransformIndex = OldTransformIndex();
        m_lastPositions[m_newTransformIndex] = transform.position;
    }

    void Update()
    {
        Vector3 newestTransform = m_lastPositions[m_newTransformIndex];
        Vector3 olderTransform = m_lastPositions[OldTransformIndex()];

        transform.position = Vector3.Lerp(olderTransform, newestTransform, InterpolationController.InterpolationFactor);
    }

    private int OldTransformIndex()
    {
        return (m_newTransformIndex == 0 ? 1 : 0);
    }
}
