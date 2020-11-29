using AgariTaku.Client.HubClients;
using AgariTaku.Client.Services;
using AgariTaku.Shared.Common;
using AgariTaku.Shared.Messages;
using AgariTaku.Shared.Types;
using AgariTaku.Tests.Common;
using FluentAssertions;
using FluentAssertions.Execution;
using Moq;
using System;
using System.Linq;
using Xunit;

namespace AgariTaku.Tests.Client
{
    public class TickServiceTests
    {
        private readonly ClientState _clientState;
        private readonly TickService _tickService;
        private readonly Mock<IGameHubClient> _clientMock;

        public TickServiceTests()
        {
            IConfiguration configuration = new TestConfiguration();
            _clientState = new ClientState(configuration);
            _tickService = new TickService(configuration, _clientState);
            _clientMock = new Mock<IGameHubClient>();
        }

        [Fact]
        public void AddTickShouldIncrementCurrentTick()
        {
            // Act
            _tickService.AddTick();

            // Assert
            _clientState.CurrentTick.Should().Be(0);
        }

        [Fact]
        public void AddTickShouldAddClientTickToBuffer()
        {
            // Act
            _tickService.AddTick();

            // Assert
            _clientState.ClientTickBuffer.Slice(0, 5).Select(tick => tick?.TickNumber).Should().Equal(0, null, null, null, null);
        }

        [Fact]
        public void AddTickShouldNotAckTicks()
        {
            // Act
            _tickService.AddTick();

            // Assert
            _clientState.AckTicks.ToArray().Should().Equal(-1, -1, -1, -1, -1);
        }

        [Fact]
        public void AddTickShouldNotAddServerTickToBuffer()
        {
            // Act
            _tickService.AddTick();

            // Assert
            using AssertionScope scope = new();
            foreach (TickSource source in (TickSource[]) Enum.GetValues(typeof(TickSource)))
            {
                _clientState.ServerTickBuffer.Slice(source, 0, 5).Select(tick => tick?.TickNumber).Should().Equal(null, null, null, null, null);
            }
        }

        [Fact]
        public void AddTickMultipleShouldIncrementCurrentTick()
        {
            // Act
            for (int i = 0; i < 13; i++)
            {
                _tickService.AddTick();
            }

            // Assert
            _clientState.CurrentTick.Should().Be(12);
        }

        [Fact]
        public void AddTickMultipleShouldAddClientTicksCircularly()
        {
            // Act
            for (int i = 0; i < 13; i++)
            {
                _tickService.AddTick();
            }

            // Assert
            _clientState.ClientTickBuffer.Slice(0, 5).Select(tick => tick?.TickNumber).Should().Equal(10, 11, 12, 8, 9);
        }

        [Fact]
        public void SendAccumulatedTicksShouldSendBufferSizeNonAckedTicks()
        {
            // Arrange
            using AssertionScope scope = new();

            int ackTickNumber = 8;
            _clientMock.Setup(m => m.ClientGameTick(It.IsAny<ClientGameTickMessage>()))
                .Callback<ClientGameTickMessage>(message =>
                {
                    message.AckTick.Should().Equal(10, ackTickNumber, 10, 10, 11);
                });

            int currentTickNumber = 12;
            _clientState.AckTicks[TickSource.Server] = 10;
            _clientState.AckTicks[TickSource.East] = ackTickNumber;
            _clientState.AckTicks[TickSource.South] = 10;
            _clientState.AckTicks[TickSource.West] = 10;
            _clientState.AckTicks[TickSource.North] = 11;
            _clientState.CurrentTick = currentTickNumber;
            for (int i = ackTickNumber + 1; i <= currentTickNumber; i++)
            {
                _clientState.ClientTickBuffer[i] = new() { TickNumber = i };
            }

            // Act
            _tickService.SendAccumulatedTicks(_clientMock.Object);

            // Assert
            _clientMock.Verify(m => m.ClientGameTick(It.IsAny<ClientGameTickMessage>()), Times.Once);
        }

        [Fact]
        public void SendAccumulatedTicksShouldSendAckedTicks()
        {
            // Arrange
            using AssertionScope scope = new();

            int ackTickNumber = 8;
            _clientMock.Setup(m => m.ClientGameTick(It.IsAny<ClientGameTickMessage>()))
                .Callback<ClientGameTickMessage>(message =>
                {
                    message.AckTick.Should().Equal(10, ackTickNumber, 10, 10, 11);
                });

            int currentTickNumber = 12;
            _clientState.AckTicks[TickSource.Server] = 10;
            _clientState.AckTicks[TickSource.East] = ackTickNumber;
            _clientState.AckTicks[TickSource.South] = 10;
            _clientState.AckTicks[TickSource.West] = 10;
            _clientState.AckTicks[TickSource.North] = 11;
            _clientState.CurrentTick = currentTickNumber;
            for (int i = ackTickNumber + 1; i <= currentTickNumber; i++)
            {
                _clientState.ClientTickBuffer[i] = new() { TickNumber = i };
            }

            // Act
            _tickService.SendAccumulatedTicks(_clientMock.Object);

            // Assert
            _clientMock.Verify(m => m.ClientGameTick(It.IsAny<ClientGameTickMessage>()), Times.Once);
        }
    }
}
