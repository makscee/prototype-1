using System;
using UnityEngine;

public class PixelRoad
{
    float T => _startTimer - Timer;

    float UTOffset(float offset)
    {
        return (T + offset) / _startTimer;
    }

    public static PixelRoad Checkerboard(Color a, Color b)
    {
        var pr = new PixelRoad(-1f);
        pr._colorFunc = (x, y) =>
        {
            var c = (x + 1000000) % 2 == (y + 1000000) % 2 ? a : b;
            return new WeightedColor(c, pr._weight);
        };
        return pr;
    }
    
    public static PixelRoad Circle(Color c, float radius, float spread, float timeScale, float timeFade, int fromX, int fromY)
    {
        var pr = new PixelRoad(timeScale + timeFade);
        pr._colorFunc = (x, y) =>
        {
            var dist = Vector2.Distance(new Vector2(x, y), new Vector2(fromX, fromY));
            var distCircle = Mathf.Lerp(0f, radius, pr.T / timeScale);
            
            if (dist > distCircle + spread) return WeightedColor.Clear;
            
            var weight = pr._weight * (1f - Mathf.Max(0, pr.T - timeScale) / timeFade);

            if (dist <= distCircle + spread)
            {
                weight = Mathf.Lerp(0f, weight, 1 - (dist - distCircle) / spread);
            }

            return new WeightedColor(c, weight);
        };
        return pr;
    }

    float Timer { get; set; }
    float _weight = 1f;
    readonly float _startTimer;

    Func<int, int, WeightedColor> _colorFunc;

    PixelRoad(float timer)
    {
        Timer = timer;
        _startTimer = timer;
    }

    public WeightedColor GetColor(int x, int y)
    {
        return _colorFunc(x, y);
    }

    public PixelRoad SetWeight(float weight)
    {
        _weight = weight;
        return this;
    }
    
    public void Update()
    {
        Timer -= Time.deltaTime;
    }

    public bool IsDone => _startTimer != -1f && Timer <= 0f;
}