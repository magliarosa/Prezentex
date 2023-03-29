import { NavLink } from "react-router-dom";
import { Button, Container, Menu } from "semantic-ui-react";
import { useStore } from "../stores/store";

export default function NavBar() {
    const {giftStore} = useStore();

    return (
        <Menu inverted fixed="top">
            <Container>
                <Menu.Item as={NavLink} to='/' header>
                    <img src="/assets/logo.png" alt="logo" style={{ marginRight: 10 }}></img>
                    Prezentex
                </Menu.Item>
                <Menu.Item as={NavLink} to='/gifts' name="Prezenty" />
                <Menu.Item name="Osoby" />
                <Menu.Item>
                    <Button as={NavLink} to='/createGift' onClick={() => giftStore.openForm()} positive content="Dodaj prezent" />
                </Menu.Item>
            </Container>
        </Menu>
    )
}