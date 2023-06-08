using System;
using System.Linq;
using UnityEngine;

class LineAnimator : MonoBehaviour
{
    private Vector3[] positions;
    private int currentIndex = 0;
    private float delta = 0;
    private static float speed = 5;
    private LineRenderer lineRenderer;

    public void SetRenderer(LineRenderer renderer)
    {
        this.lineRenderer = renderer;
        this.positions = new Vector3[renderer.positionCount];
        var length = renderer.GetPositions(this.positions);
        // remove arrow
        this.positions = this.positions.Take(length - 3).ToArray();
    }
    public bool Animate()
    {
        if (currentIndex + 1 >= positions.Length)
        {
            return false;
        }
        delta += Time.deltaTime * speed;
        if (delta > 1)
        {
            delta = 0;
            currentIndex += 1;
            if (currentIndex + 1 >= positions.Length)
            {
                return false;
            }
        }
        transform.position = Vector3.Lerp(positions[currentIndex], positions[currentIndex + 1], delta);
        return true;
    }
    public void Clear()
    {
        Destroy(lineRenderer.gameObject);
        Destroy(gameObject);
    }
    void Update()
    {
        if (!Animate())
        {
            Clear();
        }
    }
}
