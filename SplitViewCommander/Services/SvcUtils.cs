namespace SplitViewCommander.Services
{
    public class SvcUtils
    {
        public SvcUtils(){}

        /// <summary>
        /// Concatenates two arrays of the same generic type T.
        /// </summary>
        /// <typeparam name="T">Generic type T</typeparam>
        /// <param name="array1">First array to concatenate</param>
        /// <param name="array2">Seconde array to concatenate</param>
        /// <returns>Concatenated array of type T</returns>
        public T[] ConcatArrays<T>(T[] array1, T[] array2)
        {
            int totalLength = array1.Length + array2.Length;
            T[] result = new T[totalLength];

            Array.Copy(array1, result, array1.Length);
            Array.Copy(array2, 0, result, array1.Length, array2.Length);

            return result;
        }

        /// <summary>
        /// Checks if a path represents a Directory
        /// </summary>
        /// <returns>True if directory.</returns>
        public bool IsDirectory(string path)
        {
            return File.GetAttributes(path).HasFlag(FileAttributes.Directory);
        }
    }
}
