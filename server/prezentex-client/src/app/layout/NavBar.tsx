import { Button, Container, Menu } from "semantic-ui-react";
import { useStore } from "../stores/store";

export default function NavBar() {
    const {giftStore} = useStore();

    return (
        <Menu inverted fixed="top">
            <Container>
                <Menu.Item header>
                    <img src="/assets/logo.png" alt="logo" style={{ marginRight: 10 }}></img>
                    Prezentex
                </Menu.Item>
                <Menu.Item name="Prezenty" />
                <Menu.Item name="Osoby" />
                <Menu.Item>
                    <Button onClick={() => giftStore.openForm()} positive content="Dodaj prezent" />
                </Menu.Item>
            </Container>
        </Menu>
    )
}