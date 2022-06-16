// contracts/GLDToken.sol
// SPDX-License-Identifier: MIT
pragma solidity ^0.8.0;

contract SimEvent {

    event MyEvent(uint num, string text);
    string private oracle;

    constructor()
    {
    }

    function raiseMyEvent(uint n, string memory t) public {
        emit MyEvent(n, t);
    }

    function callbackFromOracle(string memory result) public {
        oracle = result;
    }

    function getOracle() view public returns(string memory) {
        return oracle;
    }
}