import React, { createContext, useContext, useEffect, useState, useCallback, useRef } from 'react';
import * as signalR from '@microsoft/signalr';

interface SignalRContextType {
  connection: signalR.HubConnection | null;
  isConnected: boolean;
  isInitialLoading: boolean;
  reconnect: () => Promise<void>;
}

const SignalRContext = createContext<SignalRContextType>({
  connection: null,
  isConnected: false,
  isInitialLoading: true,
  reconnect: async () => {},
});

export const SignalRProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [connection, setConnection] = useState<signalR.HubConnection | null>(null);
  const [isConnected, setIsConnected] = useState(false);
  const [isInitialLoading, setIsInitialLoading] = useState(true);
  const connectionRef = useRef<signalR.HubConnection | null>(null);

  const startConnection = useCallback(async (hubConnection: signalR.HubConnection) => {
    try {
      if (hubConnection.state === signalR.HubConnectionState.Disconnected) {
        await hubConnection.start();
        console.log('Connected to SignalR Hub');
        setIsConnected(true);
      }
    } catch (err) {
      console.error('SignalR Connection Error: ', err);
      setIsConnected(false);
      throw err;
    }
  }, []);

  const reconnect = useCallback(async () => {
    if (connectionRef.current) {
      await startConnection(connectionRef.current);
    }
  }, [startConnection]);

  const hubUrl = import.meta.env.VITE_SIGNALR_HUB_URL || 'http://192.168.18.152:5267/hubs/game';

  useEffect(() => {
    const newConnection = new signalR.HubConnectionBuilder()
      .withUrl(hubUrl)
      .withAutomaticReconnect()
      .build();

    connectionRef.current = newConnection;
    setConnection(newConnection);

    // SignalR lifecycle events
    newConnection.onreconnecting(() => {
      console.log('SignalR: Reconnecting...');
      setIsConnected(false);
    });

    newConnection.onreconnected(() => {
      console.log('SignalR: Reconnected');
      setIsConnected(true);
    });

    newConnection.onclose(() => {
      console.log('SignalR: Closed');
      setIsConnected(false);
    });

    // Initial startup connection
    startConnection(newConnection)
      .catch(() => {})
      .finally(() => {
        setIsInitialLoading(false);
      });

    return () => {
      newConnection.stop();
    };
  }, [startConnection]);

  return (
    <SignalRContext.Provider value={{ connection, isConnected, isInitialLoading, reconnect }}>
      {children}
    </SignalRContext.Provider>
  );
};

export const useSignalR = () => useContext(SignalRContext);