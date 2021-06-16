using System;
using System.IO;

namespace Casbin.Model
{
    public static class SyncedModelExtension
    {
        /// <summary>
        /// Creates a synced model.
        /// </summary>
        /// <returns></returns>
        public static IModel Create()
        {
            return new DefaultModel(ReaderWriterPolicyManager.Create());
        }

        /// <summary>
        /// Creates a synced model from file.
        /// </summary>
        /// <param name="path">The path of the model file.</param>
        /// <returns></returns>
        public static IModel CreateFromFile(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (File.Exists(path) is false)
            {
                throw new FileNotFoundException("Can not find the model file.");
            }

            var model = Create();
            model.LoadModelFromFile(path);
            return model;
        }

        /// <summary>
        /// Creates a default model from text.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static IModel CreateFromText(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException(nameof(text));
            }

            var model = Create();
            model.LoadModelFromText(text);
            return model;
        }
    }
}
