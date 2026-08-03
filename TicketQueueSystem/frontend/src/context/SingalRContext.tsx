import * as signalR from '@microsoft/signalr';
import { useContext, createContext, useEffect, useState } from 'react';

type SignalRContextType = {
    connection: signalR.HubConnection | null;
    isConnected: boolean;
}

const SignalRContext = createContext<SignalRContextType>({connection: null, isConnected: false});

export const SignalRProvider: React.FC<{children: React.ReactNode}> = ({ children }) => {
    const baseApiUrl = import.meta.env.VITE_API_URL

    const [connection, setConnection] = useState<signalR.HubConnection | null>(null);
    const [isConnected, setIsConnected] = useState(false);

    useEffect(() => {
        const newConnection = new signalR.HubConnectionBuilder()
            .withUrl(`${baseApiUrl}/hubs/ticket-status`)
            .withAutomaticReconnect()
            .build()
        
        setConnection(newConnection)

        newConnection.start()
            .then(() => {
                console.log("Connected to SignalR hub!")
                setIsConnected(true)
            })
            .catch(error => console.error("Connection failed: ", error))
        
        return () => {
            if(newConnection) {
                newConnection.stop()
            }
        }
    }, [])

    return (
        <SignalRContext.Provider value={{connection, isConnected}}>
            {children}
        </SignalRContext.Provider>
    )
}

export const useSignalR = () => useContext(SignalRContext);
