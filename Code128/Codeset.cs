namespace Code128
{
	/// <summary>
	/// The three code sets used by Code 128.
	/// </summary>
	internal enum Codeset
	{
		/// <summary>
		/// Set A contains mainly uppercase letters, numbers, and the special characters with ASCII values below 32.
		/// </summary>
		A = 2,

		/// <summary>
		/// Set B contains mainly lower- and uppercase letters and numbers.
		/// </summary>
		B = 1,

		/// <summary>
		/// Set C contains digit pairs.
		/// </summary>
		C = 0
	}
}