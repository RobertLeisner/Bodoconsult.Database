// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.Database.Ef.Interfaces
{
    /// <summary>
    /// Interface for handling the model data converters on app start
    /// </summary>
    public interface IModelDataConvertersHandler
    {
        /// <summary>
        /// Current unit of work
        /// </summary>
        IUnitOfWork UnitOfWork { get; }

        /// <summary>
        /// All loaded converters (sort order is important!!)
        /// </summary>
        IList<Type> Converters { get; }

        /// <summary>
        /// Messages delivered by the converters
        /// </summary>
        IList<string> Messages { get; }

        /// <summary>
        /// Load the current <see cref="IUnitOfWork"/> instance
        /// </summary>
        /// <param name="unitOfWork"></param>
        void LoadUnitOfWork(IUnitOfWork unitOfWork);

        /// <summary>
        /// Add a converter to run it later (sort order is important!!)
        /// </summary>
        /// <typeparam name="T">DbContext</typeparam>
        public void AddConverter<T>() where T : IModelDataConverter;

        /// <summary>
        /// Run all converters
        /// </summary>
        void RunConverters();
    }
}