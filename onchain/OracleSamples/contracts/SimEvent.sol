// contracts/GLDToken.sol
// SPDX-License-Identifier: MIT
pragma solidity ^0.8.0;

import "@openzeppelin/contracts/token/ERC777/ERC777.sol";

contract SimEvent {

    event MyEvent(uint num, string text);

    constructor()
    {
    }

    function raiseMyEvent(uint n, string memory t) public {
        emit MyEvent(n, t);
    }
}