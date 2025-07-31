public enum SqlOperator
{
    Equal,              // =
    NotEqual,           // <>
    GreaterThan,        // >
    GreaterThanOrEqual, // >=
    LessThan,           // <
    LessThanOrEqual,    // <=

    Like,               // LIKE or ILIKE (PostgreSQL)
    NotLike,            // NOT LIKE or NOT ILIKE (PostgreSQL)

    In,                 // IN (...)
    NotIn,              // NOT IN (...)

    Between,            // BETWEEN value1 AND value2

    IsNull,             // IS NULL
    IsNotNull           // IS NOT NULL
}
