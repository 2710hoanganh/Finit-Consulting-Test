using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace TechnicalTest.Api.Helpers;

public interface ICsvParser<T> where T : class
{
    Task<IEnumerable<T>> ParseAsync(Stream stream);
    IEnumerable<T> Parse(Stream stream);
}

public class CsvParser<T> : ICsvParser<T> where T : class
{
    public async Task<IEnumerable<T>> ParseAsync(Stream stream)
    {
        using var reader = new StreamReader(stream);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HeaderValidated = null,
            MissingFieldFound = null,
            BadDataFound = null,
            PrepareHeaderForMatch = args => args.Header.ToLower().Replace(" ", "_")
        });
        
        csv.Context.TypeConverterOptionsCache
            .GetOptions<decimal?>()
            .NullValues.Add("NULL");

        var records = new List<T>();
        await foreach (var record in csv.GetRecordsAsync<T>())
        {
            records.Add(record);
        }
        return records;
    }

    public IEnumerable<T> Parse(Stream stream)
    {
        using var reader = new StreamReader(stream);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HeaderValidated = null,
            MissingFieldFound = null,
            BadDataFound = null,
            PrepareHeaderForMatch = args => args.Header.ToLower().Replace(" ", "_")
        });
        
        csv.Context.TypeConverterOptionsCache
            .GetOptions<decimal?>()
            .NullValues.Add("NULL");

        return csv.GetRecords<T>().ToList();
    }
}
