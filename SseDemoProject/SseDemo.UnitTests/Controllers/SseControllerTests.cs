using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SseDemo.Controllers;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;


namespace SseDemo.Controllers.UnitTests
{
    /// <summary>
    /// Unit tests for <see cref="SseController"/>.
    /// </summary>
    [TestClass]
    public class SseControllerTests
    {
        /// <summary>
        /// Tests that GetCurrentTime writes data to the response body when cancellation is not immediately requested.
        /// </summary>
        [TestMethod]
        public async Task GetCurrentTime_WhenNotImmediatelyCancelled_WritesDataToResponseBody()
        {
            // Arrange
            var mockHttpContext = new Mock<HttpContext>();
            var mockResponse = new Mock<HttpResponse>();
            var mockHeaders = new Mock<IHeaderDictionary>();
            var mockStream = new Mock<Stream>();

            mockResponse.Setup(r => r.Headers).Returns(mockHeaders.Object);
            mockResponse.Setup(r => r.Body).Returns(mockStream.Object);
            mockHttpContext.Setup(c => c.Response).Returns(mockResponse.Object);

            mockStream.Setup(s => s.WriteAsync(It.IsAny<ReadOnlyMemory<byte>>(), It.IsAny<CancellationToken>()))
                .Returns(ValueTask.CompletedTask);
            mockStream.Setup(s => s.FlushAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var controller = new SseController
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                }
            };

            var cts = new CancellationTokenSource();
            cts.CancelAfter(100);

            // Act
            await controller.GetCurrentTime(cts.Token);

            // Assert
            mockStream.Verify(s => s.WriteAsync(It.IsAny<ReadOnlyMemory<byte>>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        }

        /// <summary>
        /// Tests that GetCurrentTime flushes the response body after writing data.
        /// </summary>
        [TestMethod]
        public async Task GetCurrentTime_WhenNotImmediatelyCancelled_FlushesResponseBody()
        {
            // Arrange
            var mockHttpContext = new Mock<HttpContext>();
            var mockResponse = new Mock<HttpResponse>();
            var mockHeaders = new Mock<IHeaderDictionary>();
            var mockStream = new Mock<Stream>();

            mockResponse.Setup(r => r.Headers).Returns(mockHeaders.Object);
            mockResponse.Setup(r => r.Body).Returns(mockStream.Object);
            mockHttpContext.Setup(c => c.Response).Returns(mockResponse.Object);

            mockStream.Setup(s => s.WriteAsync(It.IsAny<ReadOnlyMemory<byte>>(), It.IsAny<CancellationToken>()))
                .Returns(ValueTask.CompletedTask);
            mockStream.Setup(s => s.FlushAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var controller = new SseController
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                }
            };

            var cts = new CancellationTokenSource();
            cts.CancelAfter(100);

            // Act
            await controller.GetCurrentTime(cts.Token);

            // Assert
            mockStream.Verify(s => s.FlushAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        }

        /// <summary>
        /// Tests that GetCurrentTime completes successfully when cancellation token is already cancelled.
        /// </summary>
        [TestMethod]
        public async Task GetCurrentTime_WhenCancellationTokenAlreadyCancelled_CompletesSuccessfully()
        {
            // Arrange
            var mockHttpContext = new Mock<HttpContext>();
            var mockResponse = new Mock<HttpResponse>();
            var mockHeaders = new Mock<IHeaderDictionary>();
            var mockStream = new Mock<Stream>();

            mockResponse.Setup(r => r.Headers).Returns(mockHeaders.Object);
            mockResponse.Setup(r => r.Body).Returns(mockStream.Object);
            mockHttpContext.Setup(c => c.Response).Returns(mockResponse.Object);

            var controller = new SseController
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                }
            };

            var cts = new CancellationTokenSource();
            cts.Cancel();

            // Act
            await controller.GetCurrentTime(cts.Token);

            // Assert
            mockStream.Verify(s => s.WriteAsync(It.IsAny<ReadOnlyMemory<byte>>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        /// <summary>
        /// Tests that GetCurrentTime handles OperationCanceledException without propagating it.
        /// </summary>
        [TestMethod]
        public async Task GetCurrentTime_WhenOperationCanceledExceptionThrown_HandlesExceptionGracefully()
        {
            // Arrange
            var mockHttpContext = new Mock<HttpContext>();
            var mockResponse = new Mock<HttpResponse>();
            var mockHeaders = new Mock<IHeaderDictionary>();
            var mockStream = new Mock<Stream>();

            mockResponse.Setup(r => r.Headers).Returns(mockHeaders.Object);
            mockResponse.Setup(r => r.Body).Returns(mockStream.Object);
            mockHttpContext.Setup(c => c.Response).Returns(mockResponse.Object);

            mockStream.Setup(s => s.WriteAsync(It.IsAny<ReadOnlyMemory<byte>>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new OperationCanceledException());

            var controller = new SseController
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                }
            };

            var cts = new CancellationTokenSource();

            // Act & Assert - should not throw
            await controller.GetCurrentTime(cts.Token);
        }

        /// <summary>
        /// Tests that GetCurrentTime handles OperationCanceledException during flush operation without propagating it.
        /// </summary>
        [TestMethod]
        public async Task GetCurrentTime_WhenOperationCanceledExceptionThrownDuringFlush_HandlesExceptionGracefully()
        {
            // Arrange
            var mockHttpContext = new Mock<HttpContext>();
            var mockResponse = new Mock<HttpResponse>();
            var mockHeaders = new Mock<IHeaderDictionary>();
            var mockStream = new Mock<Stream>();

            mockResponse.Setup(r => r.Headers).Returns(mockHeaders.Object);
            mockResponse.Setup(r => r.Body).Returns(mockStream.Object);
            mockHttpContext.Setup(c => c.Response).Returns(mockResponse.Object);

            mockStream.Setup(s => s.WriteAsync(It.IsAny<ReadOnlyMemory<byte>>(), It.IsAny<CancellationToken>()))
                .Returns(ValueTask.CompletedTask);
            mockStream.Setup(s => s.FlushAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new OperationCanceledException());

            var controller = new SseController
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                }
            };

            var cts = new CancellationTokenSource();

            // Act & Assert - should not throw
            await controller.GetCurrentTime(cts.Token);
        }

        /// <summary>
        /// Tests that GetCurrentTime writes data in the correct SSE format.
        /// </summary>
        [TestMethod]
        public async Task GetCurrentTime_WhenWritingData_UsesCorrectSseFormat()
        {
            // Arrange
            var mockHttpContext = new Mock<HttpContext>();
            var mockResponse = new Mock<HttpResponse>();
            var mockHeaders = new Mock<IHeaderDictionary>();
            var mockStream = new Mock<Stream>();

            mockResponse.Setup(r => r.Headers).Returns(mockHeaders.Object);
            mockResponse.Setup(r => r.Body).Returns(mockStream.Object);
            mockHttpContext.Setup(c => c.Response).Returns(mockResponse.Object);

            byte[]? capturedBytes = null;
            mockStream.Setup(s => s.WriteAsync(It.IsAny<ReadOnlyMemory<byte>>(), It.IsAny<CancellationToken>()))
                .Callback<ReadOnlyMemory<byte>, CancellationToken>((bytes, ct) =>
                {
                    if (capturedBytes == null)
                    {
                        capturedBytes = bytes.ToArray();
                    }
                })
                .Returns(ValueTask.CompletedTask);
            mockStream.Setup(s => s.FlushAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var controller = new SseController
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                }
            };

            var cts = new CancellationTokenSource();
            cts.CancelAfter(100);

            // Act
            await controller.GetCurrentTime(cts.Token);

            // Assert
            Assert.IsNotNull(capturedBytes);
            var message = System.Text.Encoding.UTF8.GetString(capturedBytes);
            Assert.IsTrue(message.StartsWith("data: "));
            Assert.IsTrue(message.EndsWith("\n\n"));
        }

        /// <summary>
        /// Tests that GetCurrentTime continues writing until cancellation is requested.
        /// </summary>
        [TestMethod]
        public async Task GetCurrentTime_WhenRunning_ContinuesUntilCancelled()
        {
            // Arrange
            var mockHttpContext = new Mock<HttpContext>();
            var mockResponse = new Mock<HttpResponse>();
            var mockHeaders = new Mock<IHeaderDictionary>();
            var mockStream = new Mock<Stream>();

            mockResponse.Setup(r => r.Headers).Returns(mockHeaders.Object);
            mockResponse.Setup(r => r.Body).Returns(mockStream.Object);
            mockHttpContext.Setup(c => c.Response).Returns(mockResponse.Object);

            int writeCount = 0;
            var cts = new CancellationTokenSource();

            mockStream.Setup(s => s.WriteAsync(It.IsAny<ReadOnlyMemory<byte>>(), It.IsAny<CancellationToken>()))
                .Callback<ReadOnlyMemory<byte>, CancellationToken>((bytes, ct) =>
                {
                    writeCount++;
                    if (writeCount >= 3)
                    {
                        cts.Cancel();
                    }
                })
                .Returns(ValueTask.CompletedTask);
            mockStream.Setup(s => s.FlushAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var controller = new SseController
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                }
            };

            // Act
            await controller.GetCurrentTime(cts.Token);

            // Assert
            Assert.IsTrue(writeCount >= 3);
        }
    }
}