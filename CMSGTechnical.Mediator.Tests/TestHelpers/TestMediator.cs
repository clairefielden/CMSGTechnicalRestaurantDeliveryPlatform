using MediatR;

namespace CMSGTechnical.Mediator.Tests.TestHelpers;

public sealed class TestMediator : IMediator
{
    private readonly Func<object, CancellationToken, Task<object?>> _send;

    public TestMediator(Func<object, CancellationToken, Task<object?>> send)
    {
        _send = send;
    }

    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        var response = await _send(request, cancellationToken);
        return response is TResponse typed ? typed : throw new InvalidOperationException("Unexpected response type.");
    }

    public Task<object?> Send(object request, CancellationToken cancellationToken = default)
    {
        return _send(request, cancellationToken);
    }

    public Task Publish(object notification, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification
    {
        return Task.CompletedTask;
    }

    public IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    public IAsyncEnumerable<object?> CreateStream(object request, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }
}
