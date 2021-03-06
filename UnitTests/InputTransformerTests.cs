﻿using System.Collections.Generic;
using MotionRecognition;
using NUnit.Framework;

namespace UnitTests
{
	public class InputTransformerTests
	{
		string dataPath = @"../../../Data/";
		IMovementTransformer<IntervalBasedTransformerSettings> intervalTransformer;
		IMovementTransformer<IntervalBasedTransformerSettings> countTransformer;
		IMovementTransformer<ImageTransformerSettings> imageTransformer;
		IntervalBasedTransformerSettings intervalSettings;
		IntervalBasedTransformerSettings countSettings;
		ImageTransformerSettings imageSettings;

		[SetUp]
		public void Setup()
		{
			// Setup loader.
			CSVLoaderSettings settings = new CSVLoaderSettings();
			settings.filePath = dataPath + "data.csv";
			settings.trimUp = 1;
			settings.trimDown = 0;

			List<ICSVFilter> filters = new List<ICSVFilter>(1);
			ICSVFilter quaternions = new CSVEvenColumnFilter();
			filters.Add(quaternions);
			settings.filters = filters;

			var data = CSVLoader<Vector3>.LoadData(ref settings);

			// Initialize IntervalBased Transformer settings.
			intervalSettings = new IntervalBasedTransformerSettings
			{
				sampleList = data,
				interval = 4
			};
			intervalTransformer = new IntervalBasedTransformer();

			// Initialize CountBased Transformer settings.
			countSettings = new IntervalBasedTransformerSettings
			{
				sampleList = data,
				count = 10
			};
			countTransformer = new CountBasedTransformer();

			// Initialize Image Transformer.
			imageSettings = new ImageTransformerSettings
			{
				focusJoints = new LeapMotionJoint[] { LeapMotionJoint.PALM },
				samples = data,
				size = 10
			};
			imageTransformer = new ImageTransformer();
		}

		[Test]
		public void IntervalBasedTransformerReturnsValues()
		{
			Assert.IsNotEmpty(intervalTransformer.GetNeuralInput(intervalSettings));
		}

		[Test]
		public void CountBasedTransformerReturnsValues()
		{
			Assert.IsNotEmpty(countTransformer.GetNeuralInput(countSettings));
		}

		[Test]
		public void ImageTransformerReturnsValues()
		{
			Assert.IsNotEmpty(imageTransformer.GetNeuralInput(imageSettings));
		}

		[Test]
		public void TransformerReturnDifferentResults()
		{
			Assert.AreNotEqual(intervalTransformer.GetNeuralInput(intervalSettings), countTransformer.GetNeuralInput(countSettings));
		}

		[Test]
		public void TransformerReturnEqualResults()
		{
			// 59 rows in data file, a run with count 5 should equal a run with an interval of 11.8 (59 / 5 = 11.8)
			countSettings.count = 5;
			double[] countTransformerResult = countTransformer.GetNeuralInput(countSettings);

			intervalSettings.interval = 59d / 5d;
			double[] intervalTransformerResult = intervalTransformer.GetNeuralInput(intervalSettings);

			Assert.AreEqual(countTransformerResult, intervalTransformerResult);
		}

		// ImageFactory
		[Test]
		public void ImageTransformerReturns3DImage()
		{
			var image = imageTransformer.GetNeuralInput(imageSettings);
			// One dimensional Image of top and front view.
			uint expectedLength = imageSettings.size * imageSettings.size * 2;
			
			Assert.IsTrue(image.Length == expectedLength);
		}
	}
}
