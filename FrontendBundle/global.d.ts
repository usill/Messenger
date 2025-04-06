import { HubConnection } from "@microsoft/signalr";
import { Messanger } from "./pages/messanger/types/Messanger";
import { Contact } from "./pages/messanger/types/Contact";

export {};

declare global {
    interface Window {
        sendLogin: (button: HTMLButtonElement) => void,
        registration: (button: HTMLButtonElement) => void,
        preventDefaultFor: (event: Event) => void,
        clearError: (input: HTMLInputElement) => void,
        connection: HubConnection,
        chatProxy: Messanger,
        contacts: Contact[],
        openModal: (id: string) => void,
        closeModal: (id: string) => void,
        stopPropagationFor: (event: Event) => void,
        logout: () => void,
        sendMessage: () => void,
        textAreaInput: (event: KeyboardEvent) => void,
        findContactByForm: (button: HTMLButtonElement) => void,
        findContact: (login: string) => void
    }
}