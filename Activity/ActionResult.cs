// -----------------------------------------------------------------------
// <copyright file="ActionResult.cs" company="Sbrinna">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------
namespace GisoFramework.Activity
{
    /// <summary>
    /// Implements a class that represents the result of an operation
    /// </summary>
    public class ActionResult
    {
        /// <summary>
        /// Gets or sets a value indicating whether if the action has is success or fail
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the message of result
        /// </summary>
        public string MessageError { get; set; }
    }
}
