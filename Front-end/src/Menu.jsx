import { Link } from 'react-router-dom';
import '../styles/body-style.css'
function Menu() {
    return (
        <div className="menu-box ">
            <Link className='Link' to={"/"}><div className='menu-item '>Dar de alta</div></Link>
            <Link className='Link' to={"/productos"}><div className='menu-item '>Comprar</div></Link>
        </div>
    );
}

export default Menu;