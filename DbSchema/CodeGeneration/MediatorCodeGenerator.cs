using System;

namespace DbSchema.CodeGeneration {

    public static class Str {

        public static string FirstLetterUpperCase(this string value) {

            if(value.Length == 0) {
                return value;
            }
            if(!char.IsUpper(value[0]) && value.Length > 1) {
                char c = char.ToUpper(value[0]);
                value = string.Concat(new Span<char>(ref c), value.AsSpan(1));
            }
            return value;
        }
        public static string FirstLetterLowerCase(this string value) {

            if(value.Length == 0) {
                return value;
            }
            if(!char.IsLower(value[0]) && value.Length > 1) {
                char c = char.ToLower(value[0]);
                value = string.Concat(new Span<char>(ref c), value.AsSpan(1));
            }
            return value;
        }
    }
}
























/* Examples

    public class CreateCustomerRequest : IRequest<Response> {

        public Customer Customer { get; set; }

        public CreateCustomerRequest(Customer customer) {
            Customer = customer;
        }
    }

    public class UpdateCustomerRequest : IRequest<Response> {

        public Customer Customer { get; set; }

        public UpdateCustomerRequest(Customer customer) {
            Customer = customer;
        }
    }

    public class DeleteCustomerRequest : IRequest<Response> {

        public StringKey<ICustomer> CustomerId { get; set; }

        public DeleteCustomerRequest(StringKey<ICustomer> customerId) {
            CustomerId = customerId;
        }
    }

    public class LoadCustomerRequest : IRequest<Response<Customer>> {

        public StringKey<ICustomer> CustomerId { get; set; }

        public LoadCustomerRequest(StringKey<ICustomer> customerId) {
            CustomerId = customerId;
        }
    }

    public class LoadCustomersRequest : IRequest<IList<Customer>> {

    }

     public class CreateCustomerHandler : IRequestHandler<CreateCustomerRequest, Response> {

        private static readonly CustomerValidator _validator = new CustomerValidator();

        private static readonly IPreparedInsertQuery<Customer> _insertQuery;

        static CreateCustomerHandler() {

            CustomerTable table = CustomerTable.Instance;

            _insertQuery = Query.Prepare<Customer>()
                .Insert(table)
                .Values(values => values
                    .Set(table.CustomerId, info => info.CustomerId)
                    .Set(table.CompanyName, info => info.CompanyName)
                    .Set(table.ContactName, info => info.ContactName)
                    .Set(table.ContactTitle, info => info.ContactTitle)
                    .Set(table.Address, info => info.Address)
                    .Set(table.City, info => info.City)
                    .Set(table.Region, info => info.Region)
                    .Set(table.PostalCode, info => info.PostalCode)
                    .Set(table.Country, info => info.Country)
                    .Set(table.Phone, info => info.Phone)
                    .Set(table.Fax, info => info.Fax)
                ).Build();
        }

        private readonly IDatabase _database;

        public CreateCustomerHandler(IDatabase database) {
            _database = database;
        }

        public async Task<Response> Handle(CreateCustomerRequest request, CancellationToken cancellationToken) {

            FluentValidation.Results.ValidationResult validation = _validator.Validate(request.Customer);

            if(!validation.IsValid) {
                return Response.Failed(validation.Errors);
            }

            using(Transaction transaction = new Transaction(_database)) {

                NonQueryResult result = await _insertQuery.ExecuteAsync(request.Customer, transaction, cancellationToken, TimeoutLevel.ShortInsert);

                if(result.RowsEffected != 1) {
                    throw new Exception($"{nameof(result.RowsEffected)} != 1. Value = {result.RowsEffected}");
                }
                transaction.Commit();
            }
            return Response.Successful;
        }
    }

    public class UpdateCustomerHandler : IRequestHandler<UpdateCustomerRequest, Response> {

        private static readonly CustomerValidator _validator = new CustomerValidator();

        private static readonly IPreparedUpdateQuery<Customer> _updateQuery;

        static UpdateCustomerHandler() {

            CustomerTable table = CustomerTable.Instance;

            _updateQuery = Query.Prepare<Customer>()
                .Update(table)
                .Values(values => values
                    .Set(table.CompanyName, info => info.CompanyName)
                    .Set(table.ContactName, info => info.ContactName)
                    .Set(table.ContactTitle, info => info.ContactTitle)
                    .Set(table.Address, info => info.Address)
                    .Set(table.City, info => info.City)
                    .Set(table.Region, info => info.Region)
                    .Set(table.PostalCode, info => info.PostalCode)
                    .Set(table.Country, info => info.Country)
                    .Set(table.Phone, info => info.Phone)
                    .Set(table.Fax, info => info.Fax)
                )
                .Where(where => where.EQUALS(table.CustomerId, info => info.CustomerId))
                .Build();
        }

        private readonly IDatabase _database;

        public UpdateCustomerHandler(IDatabase database) {
            _database = database;
        }

        public async Task<Response> Handle(UpdateCustomerRequest request, CancellationToken cancellationToken) {

            FluentValidation.Results.ValidationResult validation = _validator.Validate(request.Customer);

            if(!validation.IsValid) {
                return Response.Failed(validation.Errors);
            }

            using(Transaction transaction = new Transaction(_database)) {

                NonQueryResult result = await _updateQuery.ExecuteAsync(request.Customer, transaction, cancellationToken, TimeoutLevel.ShortUpdate);

                if(result.RowsEffected != 1) {
                    throw new Exception($"{nameof(result.RowsEffected)} != 1. Value = {result.RowsEffected}");
                }
                transaction.Commit();
            }
            return Response.Successful;
        }
    }

    public class DeleteCustomerHandler : IRequestHandler<DeleteCustomerRequest, Response> {

        private static readonly IPreparedDeleteQuery<StringKey<ICustomer>> _deleteQuery;

        static DeleteCustomerHandler() {

            CustomerTable table = CustomerTable.Instance;

            _deleteQuery = Query.Prepare<StringKey<ICustomer>>()
                .Delete(table)
                .Where(where => where.EQUALS(table.CustomerId, customerId => customerId))
                .Build();
        }

        private readonly IDatabase _database;

        public DeleteCustomerHandler(IDatabase database) {
            _database = database;
        }

        public async Task<Response> Handle(DeleteCustomerRequest request, CancellationToken cancellationToken) {

            if(!request.CustomerId.IsValid) {
                return Response.Failed($"{request.CustomerId} must be valid");
            }

            using(Transaction transaction = new Transaction(_database)) {

                NonQueryResult result = await _deleteQuery.ExecuteAsync(request.CustomerId, transaction, cancellationToken, TimeoutLevel.ShortDelete);

                if(result.RowsEffected != 1) {
                    throw new Exception($"{nameof(result.RowsEffected)} != 1. Value = {result.RowsEffected}");
                }
                transaction.Commit();
            }
            return Response.Successful;
        }
    }

    public class LoadCustomerHandler : IRequestHandler<LoadCustomerRequest, Response<Customer>> {

        private static readonly IPreparedQueryExecute<StringKey<ICustomer>, Customer> _selectQuery;

        static LoadCustomerHandler() {

            CustomerTable table = CustomerTable.Instance;

            _selectQuery = Query.Prepare<StringKey<ICustomer>>()
                .Select(row => new Customer(table, row))
                .From(table)
                .Where(where => where.EQUALS(table.CustomerId, customerId => customerId))
                .Build();
        }

        private readonly IDatabase _database;

        public LoadCustomerHandler(IDatabase database) {
            _database = database;
        }

        public async Task<Response<Customer>> Handle(LoadCustomerRequest request, CancellationToken cancellationToken) {

            if(!request.CustomerId.IsValid) {
                throw new Exception($"{nameof(request.CustomerId)} must be valid");
            }

            QueryResult<Customer> result = await _selectQuery.ExecuteAsync(request.CustomerId, _database, cancellationToken, TimeoutLevel.ShortSelect);

            if(result.Rows.Count != 1) {
                return Response<Customer>.Failed($"{nameof(Customer)} record not found.");
            }
            return Response<Customer>.Successful(result.Rows[0]);
        }
    }

    public class LoadCustomersHandler : IRequestHandler<LoadCustomersRequest, IList<Customer>> {

        private static readonly IPreparedQueryExecute<bool, Customer> _selectQuery;

        static LoadCustomersHandler() {

            CustomerTable table = CustomerTable.Instance;

            _selectQuery = Query.Prepare<bool>()
                .Select(row => new Customer(table, row))
                .From(table)
                .Build();
        }

        private readonly IDatabase _database;

        public LoadCustomersHandler(IDatabase database) {
            _database = database;
        }

        public async Task<IList<Customer>> Handle(LoadCustomersRequest request, CancellationToken cancellationToken) {

            QueryResult<Customer> result = await _selectQuery.ExecuteAsync(true, _database, cancellationToken, TimeoutLevel.ShortSelect);

            return result.Rows;
        }
    }
*/
