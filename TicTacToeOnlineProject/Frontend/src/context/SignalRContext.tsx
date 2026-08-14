// src/context/SignalRContext.tsx
import React, { createContext, useContext, useEffect, useState } from 'react';
import * as signalR from '@microsoft/signalr';

interface SignalRContextType {
  connection: signalR.HubConnection | null;
  isConnected: boolean;
}

const SignalRContext = createContext<SignalRContextType>({
  connection: null,
  isConnected: false,
});

export const SignalRProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [connection, setConnection] = useState<signalR.HubConnection | null>(null);
  const [isConnected, setIsConnected] = useState(false);

  useEffect(() => {
    // 1. Build connection instance
    const newConnection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:5267/hubs/game')
      .withAutomaticReconnect()
      .build();

    setConnection(newConnection);

    // 2. Start connection
    newConnection
      .start()
      .then(() => {
        console.log('Connected to SignalR Hub');
        setIsConnected(true);
      })
      .catch((err) => console.error('SignalR Connection Error: ', err));

    // 3. Clean up on unmount
    return () => {
      newConnection.stop();
    };
  }, []);

  return (
    <SignalRContext.Provider value={{ connection, isConnected }}>
      {children}
    </SignalRContext.Provider>
  );
};

// Custom Hook for easier usage across components
export const useSignalR = () => useContext(SignalRContext);