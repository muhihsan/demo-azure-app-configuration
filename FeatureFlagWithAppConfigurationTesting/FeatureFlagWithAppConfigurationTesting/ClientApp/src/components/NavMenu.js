import React from 'react';
import { Collapse, Container, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';
import { connect } from 'react-redux';
import { actionCreators } from '../store/WeatherForecasts';
import { bindActionCreators } from 'redux';
import './NavMenu.css';

class NavMenu extends React.Component {
  constructor (props) {
    super(props);

    this.toggle = this.toggle.bind(this);
    this.state = {
      isOpen: false
    };
  }

  componentDidMount() {
    // This method is called when the component is first added to the document
    this.props.getWeatherForecastFeatureFlag();
  }

  toggle () {
    this.setState({
      isOpen: !this.state.isOpen
    });
  }
  render () {
    const navItems = (<ul className="navbar-nav flex-grow">
        <NavItem>
            <NavLink tag={Link} className="text-dark" to="/">Home</NavLink>
        </NavItem>
        <NavItem>
            <NavLink tag={Link} className="text-dark" to="/counter">Counter</NavLink>
        </NavItem>
        {
            this.props.isEnabled ?
                <NavItem>
                    <NavLink tag={Link} className="text-dark" to="/fetch-data">Fetch data</NavLink>
                </NavItem> : null
        }
    </ul>);

    return (
      <header>
        <Navbar className="navbar-expand-sm navbar-toggleable-sm border-bottom box-shadow mb-3" light >
          <Container>
            <NavbarBrand tag={Link} to="/">FeatureFlagWithAppConfigurationTesting Telstra</NavbarBrand>
            <NavbarToggler onClick={this.toggle} className="mr-2" />
            <Collapse className="d-sm-inline-flex flex-sm-row-reverse" isOpen={this.state.isOpen} navbar>
              { this.props.isLoadingFeatureFlag ? null : navItems }
            </Collapse>
          </Container>
        </Navbar>
      </header>
    );
  }
}

export default connect(
  state => state.weatherForecasts,
  dispatch => bindActionCreators(actionCreators, dispatch)
)(NavMenu);
