using System;
using System.Threading;
using System.Threading.Tasks;

using CMS.DataEngine;
using CMS.Helpers;

namespace DancingGoat.Commerce;

/// <summary>
/// Service responsible for generating unique order numbers.
/// </summary>
public sealed class OrderNumberGenerator
{
    private const string SEQUENCE_NAME = "OrderNumberSequence";


    /// <summary>
    /// Generates a new unique order number using the underlying SQL sequence.
    /// The format is <c>ORD#NNNNNN</c>, where <c>NNNNNN</c> is a zero-padded
    /// sequential value retrieved from the database.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the generated order number string.
    /// </returns>
    public async Task<string> GenerateOrderNumber(CancellationToken cancellationToken)
    {
        var sequence = await GetNextOrderSequenceNumber(cancellationToken);
        return $"ORD#{sequence:D6}";
    }


    /// <summary>
    /// Retrieves the next value from the configured SQL sequence.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the next numeric value from the sequence.
    /// </returns>
    private static async Task<long> GetNextOrderSequenceNumber(CancellationToken cancellationToken)
    {
        var query = $"SELECT NEXT VALUE FOR [{SEQUENCE_NAME}];";

        var scalar = await ConnectionHelper.ExecuteScalarAsync(query,
            null,
            QueryTypeEnum.SQLQuery,
            cancellationToken);

        return ValidationHelper.GetLong(scalar, 0);
    }


    /// <summary>
    /// Ensures that the SQL sequence required for order number generation exists.
    /// If the sequence does not exist, it will be created.
    /// </summary>
    public void Initialize()
    {
        if (!SequenceExists())
        {
            CreateSequence();
        }
    }


    /// <summary>
    /// Checks whether the SQL sequence used for generating order numbers already exists in the database.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the sequence exists; otherwise, <c>false</c>.
    /// </returns>
    private static bool SequenceExists()
    {
        const string QUERY = "SELECT COUNT(*) FROM sys.sequences WHERE name = @sequenceName;";

        var count = ValidationHelper.GetInteger(
            ConnectionHelper.ExecuteScalar(QUERY,
                new QueryDataParameters
                {
                    { "@sequenceName", SEQUENCE_NAME }
                },
                QueryTypeEnum.SQLQuery),
            0);

        return count > 0;
    }


    /// <summary>
    /// Creates the SQL sequence used for generating order numbers.
    /// The sequence starts at 1, increments by 1, and uses a cache of 100 values.
    /// </summary>
    private static void CreateSequence()
    {
        const string query = $"CREATE SEQUENCE [{SEQUENCE_NAME}] AS BIGINT START WITH 1 INCREMENT BY 1 MINVALUE 1 NO CYCLE CACHE 100;";

        ConnectionHelper.ExecuteNonQuery(query,
            null,
            QueryTypeEnum.SQLQuery);
    }
}
