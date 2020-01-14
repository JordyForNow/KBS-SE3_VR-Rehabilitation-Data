﻿﻿using MotionRecognition;
using MotionRecognitionHelper;
using System;

namespace Sandbox
{
	class Program
	{
		static void Main(string[] args)
		{
			NetworkContainer container = new NetworkContainer
			{
				verbose = true
			};

			ImageNetworkTrainSettings trainSettings = new ImageNetworkTrainSettings
			{
				correctInputDirectory = @"../../../CorrectTestData",
				incorrectInputDirectory = @"../../../IncorrectTestData",

				outputDirectory = @"../../../DataOut",
				outputName = "ModelTwo",

				sampleCount = 10
			};

			ImageNetworkTrainController.PrepareData(ref trainSettings, ref container);
			ImageNetworkTrainController.PrepareNetwork(ref trainSettings, ref container);
			ImageNetworkTrainController.Train(ref trainSettings, ref container);

			ImageNetworkPredictSettings predictSettings = new ImageNetworkPredictSettings
			{
				trainedNetwork = @"../../../DataOut/ModelTwo.eg",
				predictData = @"../../../testCor/0-2.csv",

				networkInputSize = 10
			};

			ImageNetworkPredictController.PreparePredictor(ref predictSettings, ref container);
			Console.WriteLine(ImageNetworkPredictController.Predict(ref predictSettings, ref container));
		}
	}
}

//using System;
//using System.IO;
//using MotionRecognition;

//namespace Sandbox
//{
//	class Program
//	{
//		static void Main(string[] args)
//		{
//			NetworkContainer container = new NetworkContainer
//			{
//				verbose = true
//			};

//			CountNetworkTrainSettings trainSettings = new CountNetworkTrainSettings
//			{
//				correctInputDirectory = @"../../../CorrectTestData",
//				incorrectInputDirectory = @"../../../IncorrectTestData",

//				outputDirectory = @"../../../DataOut",
//				outputName = "ModelOne",

//				sampleCount = 10
//			};

//			CountNetworkTrainController.prepareData(ref trainSettings, ref container);
//			CountNetworkTrainController.prepareNetwork(ref trainSettings, ref container);
//			CountNetworkTrainController.train(ref trainSettings, ref container);

//			CountNetworkPredictSettings predictSettings = new CountNetworkPredictSettings
//			{
//				trainedNetwork = @"../../../DataOut/ModelOne.eg",
//				predictData = @"../../../TestCor/0-2.csv",

//				networkInputSize = 10
//			};

//			CountNetworkPredictController.preparePredictor(ref predictSettings, ref container);
//			Console.WriteLine(CountNetworkPredictController.predict(ref predictSettings, ref container));
//		}
//	}
//}

