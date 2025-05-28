// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using System.Diagnostics;
using Bodoconsult.App.Interfaces;
using Bodoconsult.Database.Ef.Interfaces;

namespace Bodoconsult.Database.Ef.MigrationTools;

/// <summary>
/// Handles the model data converters on app start
/// </summary>
public class ModelDataConvertersHandler : IModelDataConvertersHandler
{
    

    private readonly IAppLoggerProxy _logger;

    /// <summary>
    /// Default ctor
    /// </summary>
    public ModelDataConvertersHandler(IAppLoggerProxy logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }


    /// <summary>
    /// Current unit of work
    /// </summary>
    public IUnitOfWork UnitOfWork { get; private set; }

    /// <summary>
    /// All loaded converters (sort order is important!!)
    /// </summary>
    public IList<Type> Converters { get; } = new List<Type>();

    /// <summary>
    /// Messages delivered by the converters
    /// </summary>
    public IList<string> Messages { get; } = new List<string>();

    public void LoadUnitOfWork(IUnitOfWork unitOfWork)
    {
        UnitOfWork = unitOfWork;
    }

    /// <summary>
    /// Add a converter to run it later
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void AddConverter<T>() where T : IModelDataConverter
    {
        Converters.Add(typeof(T));
    }

    public void RunConverters()
    {

        if (Converters.Count <= 0)
        {
            return;
        }


        var stopWatch = new Stopwatch();

        foreach (var converterType in Converters)
        {
            var msg = $"{converterType.Name}: started...";
            Debug.Print(msg);

            _logger.LogInformation(msg);

            RunConverter(converterType);


            stopWatch.Restart();

            RunConverter(converterType);

            stopWatch.Stop();

            msg = $"{converterType.Name}: ended after {stopWatch.ElapsedMilliseconds / 1000}s";
            _logger.LogInformation(msg);
        }
    }

    /// <summary>
    /// Run a converter. Internal usage for testing only
    /// </summary>
    /// <param name="converterType">Type name of the converter</param>
    public void RunConverter(Type converterType)
    {
        var conv = (IModelDataConverter)Activator.CreateInstance(converterType, UnitOfWork, _logger);

        if (conv == null)
        {
            throw new ArgumentException($"{nameof(conv)} may not be null");
        }

        var convName = conv.GetType().Name;

        Debug.WriteLine(convName);

        var stopWatch = new Stopwatch();
        stopWatch.Start();

        if (conv.CheckPremisesToRunConverter())
        {
            conv.Run();
        }

        AddMessages(conv);

        stopWatch.Stop();
        Debug.WriteLine($"Done: {convName}: {stopWatch.ElapsedMilliseconds / 1000} ms");
    }

    private void AddMessages(IModelDataConverter conv)
    {

        if (conv.Messages.Count == 0)
        {
            return;
        }

        var name = conv.GetType().Name;

        foreach (var msg in conv.Messages)
        {
            Messages.Add($"{name}:{msg}");
        }
    }
}