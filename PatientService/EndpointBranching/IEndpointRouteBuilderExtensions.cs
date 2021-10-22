namespace PatientService;

public static class IEndpointRouteBuilderExtensions
{
    public static BranchedEndpointRouteBuilder Path(this IEndpointRouteBuilder app
        , string path
        , Action<BranchedEndpointRouteBuilder> child)
    {
        return new BranchedEndpointRouteBuilder(app, path, child);
    }
    
    public class BranchedEndpointRouteBuilder
    {
        private static readonly string[] GET = {"GET"};
        private static readonly string[] PUT = {"PUT"};
        private static readonly string[] DELETE = {"DELETE"};

        private readonly IEndpointRouteBuilder _app;
        private readonly string _path;

        internal BranchedEndpointRouteBuilder(IEndpointRouteBuilder app
            , string path
            , Action<BranchedEndpointRouteBuilder> routes)
        {
            _app = app;
            _path = path;

            routes(this);
        }

        public BranchedEndpointRouteBuilder Path(string path, Action<BranchedEndpointRouteBuilder> routes) =>
            new BranchedEndpointRouteBuilder(_app, CombinePath(_path, path), routes);

        public RouteHandlerBuilder MapMethods(IEnumerable<string> methods, Delegate requestDelegate) =>
            _app.MapMethods(_path, methods, requestDelegate);

        public RouteHandlerBuilder MapGet(Delegate requestDelegate) => MapMethods(GET, requestDelegate);

        public RouteHandlerBuilder MapPut(Delegate requestDelegate) => MapMethods(PUT, requestDelegate);

        public RouteHandlerBuilder MapDelete(Delegate requestDelegate) => MapMethods(DELETE, requestDelegate);

        private string CombinePath(params string[] paths) =>  String.Join('/', paths);
    }
}

