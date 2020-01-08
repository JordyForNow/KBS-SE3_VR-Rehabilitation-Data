using System;
using System.Linq;

namespace MotionRecognition
{
	public enum LeapMotionJoint
	{
		PALM = 0,
		THUMB_0 = 1,
		THUMB_1 = 2,
		THUMB_2 = 3,
		THUMB_3 = 4,
		INDEX_0 = 5,
		INDEX_1 = 6,
		INDEX_2 = 7,
		INDEX_3 = 8,
		MIDDLE_0 = 9,
		MIDDLE_1 = 10,
		MIDDLE_2 = 11,
		MIDDLE_3 = 12,
		RING_0 = 13,
		RING_1 = 14,
		RING_2 = 15,
		RING_3 = 16,
		BABY_0 = 17,
		BABY_1 = 18,
		BABY_2 = 19,
		BABY_3 = 20
	}

	public struct ImageTransformerSettings
	{
		public int size;
		public Sample<Vec3>[] samples;
		public LeapMotionJoint[] focus_joints;
	}

	/*
	* Transforms sample list to 3d-matrix.
	*/
	public class ImageTransformer : IMovementTransformer<ImageTransformerSettings>
	{
		private float Remap(float value, float from1, float to1, float from2, float to2)
		{
			return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
		}
		public double[] GetNeuralInput(ImageTransformerSettings settings)
		{
			double[] dField = new double[settings.size * settings.size * 2];

			int incr = (int)Math.Floor((decimal)int.MaxValue / settings.focus_joints.Length);

			Vec3 vecMin = new Vec3();
			Vec3 vecMax = new Vec3();
			foreach (var s in settings.samples)
			{
				foreach (var m in s.values)
				{
					vecMin.x = m.x < vecMin.x ? m.x : vecMin.x;
					vecMin.y = m.y < vecMin.y ? m.y : vecMin.y;
					vecMin.z = m.z < vecMin.z ? m.z : vecMin.z;

					vecMax.y = m.y > vecMax.y ? m.y : vecMax.y;
					vecMax.x = m.x > vecMax.x ? m.x : vecMax.x;
					vecMax.z = m.z > vecMax.z ? m.z : vecMax.z;
				}
			}

			// Remap all sample vectors to a map in a range from 0 -> 499 (500).
			foreach (var sample in settings.samples)
			{
				for (int i = 0; i < sample.values.Length; i++)
				{
					if (settings.focus_joints.Count(o => (int)o == i) > 0)
					{
						int x = (int)Math.Round(Remap(sample.values[i].x, vecMin.x, vecMax.x, 0, settings.size - 1));
						int y = (int)Math.Round(Remap(sample.values[i].y, vecMin.y, vecMax.y, 0, settings.size - 1));
						int z = (int)Math.Round(Remap(sample.values[i].z, vecMin.z, vecMax.z, 0, settings.size - 1));

						dField[(settings.size * y) + x] += incr;
						dField[(settings.size * settings.size) + (settings.size * z) + x] += incr;
					}
				}
			}
			return dField;
		}
	}
}
