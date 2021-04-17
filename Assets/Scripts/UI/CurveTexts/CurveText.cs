using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class CurveText : Text
{
	public AnimationCurve curve;
	public float scale = 100;

	protected override void OnPopulateMesh(VertexHelper vh)
	{
		base.OnPopulateMesh(vh);

		List<UIVertex> stream = new List<UIVertex>();
		vh.GetUIVertexStream(stream);

		if (!stream.Any())
			return;

		var thisWidth = this.rectTransform.rect.width;
		var textWidth = stream.Max(v => v.position.x) - stream.Min(v => v.position.x);
		if (textWidth > thisWidth)
			textWidth = thisWidth;

		for (int i = 0; i < stream.Count; i++)
		{
			UIVertex v = stream[i];

			var xIndex = v.position.x;
			var curveAtPoint = curve.Evaluate(xIndex / thisWidth);
			v.position += Vector3.up * curveAtPoint * scale;

			stream[i] = v;
		}

		vh.AddUIVertexTriangleStream(stream);
	}
}
