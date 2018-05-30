import * as React from 'react';
import styled from 'styled-components';
import { Card as CardData } from 'Features/cards/reducer';

const CardContainer = styled.div`
  display: inline-block;
  border: solid 1px;
  font-size: 12px;
  position: absolute;
  transition: all 1.5s ease-in-out;
  left: 0;
  top: 0;
  border-radius: 2px;
  box-shadow: 0px 0px 1px 1px grey;
  font-family: sans-serif;
  width: 60px;
  height: 100px;
  background: white;
`;

const TappedCardContainer = CardContainer.extend`
  transform-origin: top left;
  transform: rotate(90deg) translateY(-100%);
`;

export const Card = (props: CardData): JSX.Element => {
  const Container = props.tapped ? TappedCardContainer : CardContainer;

  let style = {
    'left': props.screenX,
    'top': props.screenY
  };

  return (
    <Container style={style} key={'card-' + props.id}>
      <div>{props.name}</div>
      <div>hp {props.alive ? props.currentHp + '/' : ''} {props.maxHp}</div>
      <div>damage {props.damage}</div>
      <div>manna {props.mannaCost}</div>
    </Container>
  );
};

export default Card;
