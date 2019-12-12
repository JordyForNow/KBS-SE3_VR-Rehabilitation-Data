﻿using Keras;
using Keras.Layers;
using Keras.Models;
using Numpy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Keras.Datasets;
using Keras.Optimizers;
using Python.Runtime;
using Keras.Utils;

namespace MotionRecognition
{
	public class Neural
	{

        public static void Run()
        {
            // TODO: make relative
            Environment.SetEnvironmentVariable("PATH", @"C:\Users\buste\AppData\Local\Programs\Python\Python36", EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("PYTHONHOME", @"C:\Users\buste\AppData\Local\Programs\Python\Python36\python.exe", EnvironmentVariableTarget.Process);

            //Load train data
            NDarray x = np.array(new float[,] { { 0, 0 }, { 0, 1 }, { 1, 0 }, { 1, 1 } });
            NDarray y = np.array(new float[] { 0, 1, 1, 0 });

            //Build sequential model
            var model = new Sequential();
            model.Add(new Dense(32, activation: "relu", input_shape: new Shape(2)));
            model.Add(new Dense(64, activation: "relu"));
            model.Add(new Dense(1, activation: "sigmoid"));

            //Compile and train
            model.Compile(optimizer: "sgd", loss: "binary_crossentropy", metrics: new string[] { "accuracy" });
            model.Fit(x, y, batch_size: 2, epochs: 1000, verbose: 1);

            //Save model and weights
            string json = model.ToJson();
            File.WriteAllText("model.json", json);
            model.SaveWeight("model.h5");

            //Load model and weight
            var loaded_model = Sequential.ModelFromJson(File.ReadAllText("model.json"));
            loaded_model.LoadWeight("model.h5");
        }

        public static void Run2()
        {
            int batch_size = 128;
            int num_classes = 10;
            int epochs = 1;

            // input image dimensions
            int img_rows = 28, img_cols = 28;

            Shape input_shape = null;

            // the data, split between train and test sets
            var ((x_train, y_train), (x_test, y_test)) = MNIST.LoadData();

            if (Backend.ImageDataFormat() == "channels_first")
            {
                x_train = x_train.reshape(x_train.shape[0], 1, img_rows, img_cols);
                x_test = x_test.reshape(x_test.shape[0], 1, img_rows, img_cols);
                input_shape = (1, img_rows, img_cols);
            }
            else
            {
                x_train = x_train.reshape(x_train.shape[0], img_rows, img_cols, 1);
                x_test = x_test.reshape(x_test.shape[0], img_rows, img_cols, 1);
                input_shape = (img_rows, img_cols, 1);
            }

            x_train = x_train.astype(np.float32);
            x_test = x_test.astype(np.float32);
            x_train /= 255;
            x_test /= 255;
            Console.WriteLine($"x_train shape: {x_train.shape}");
            Console.WriteLine($"{x_train.shape[0]} train samples");
            Console.WriteLine($"{x_test.shape[0]} test samples");

            // convert class vectors to binary class matrices
            y_train = Util.ToCategorical(y_train, num_classes);
            y_test = Util.ToCategorical(y_test, num_classes);

            // Build CNN model
            var model = new Sequential();
            model.Add(new Conv2D(32, kernel_size: (3, 3).ToTuple(),
                                    activation: "relu",
                                    input_shape: input_shape));
            model.Add(new Conv2D(64, (3, 3).ToTuple(), activation: "relu"));
            model.Add(new MaxPooling2D(pool_size: (2, 2).ToTuple()));
            model.Add(new Dropout(0.25));
            model.Add(new Flatten());
            model.Add(new Dense(128, activation: "relu"));
            model.Add(new Dropout(0.5));
            model.Add(new Dense(num_classes, activation: "softmax"));

            model.Compile(loss: "categorical_crossentropy",
                optimizer: new Adadelta(), metrics: new string[] { "accuracy" });

            model.Fit(x_train, y_train,
                        batch_size: batch_size,
                        epochs: epochs,
                        verbose: 1,
                        validation_data: new NDarray[] { x_test, y_test });
            var score = model.Evaluate(x_test, y_test, verbose: 0);
            Console.WriteLine($"Test loss: {score[0]}");
            Console.WriteLine($"Test accuracy: {score[1]}");
        }

    }
}
